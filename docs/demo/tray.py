# A tiny StatusNotifier host for a headless X session: registers apps' tray icons and clicks their
# menu items over D-Bus (dbusmenu), the way a desktop panel would.
#   tray.py serve                  run the watcher (keeps running)
#   tray.py click "<menu label>"   click a menu item of the first registered tray icon
import asyncio, os, sys
from dbus_next.aio import MessageBus
from dbus_next.service import ServiceInterface, method, dbus_property, PropertyAccess, signal
from dbus_next import Variant, BusType

STATE = os.path.join(os.environ.get("XDG_RUNTIME_DIR") or "/tmp", "shotora-demo-sni-items")


class Watcher(ServiceInterface):
    def __init__(self):
        super().__init__("org.kde.StatusNotifierWatcher")
        self.items = []

    @method()
    def RegisterStatusNotifierItem(self, service: "s"):
        self.items.append(service)
        open(STATE, "a").write(service + "\n")
        self.StatusNotifierItemRegistered(service)

    @method()
    def RegisterStatusNotifierHost(self, service: "s"):
        pass

    @signal()
    def StatusNotifierItemRegistered(self, service) -> "s":
        return service

    @dbus_property(access=PropertyAccess.READ)
    def RegisteredStatusNotifierItems(self) -> "as":
        return self.items

    @dbus_property(access=PropertyAccess.READ)
    def IsStatusNotifierHostRegistered(self) -> "b":
        return True

    @dbus_property(access=PropertyAccess.READ)
    def ProtocolVersion(self) -> "i":
        return 0


async def serve():
    bus = await MessageBus(bus_type=BusType.SESSION).connect()
    bus.export("/StatusNotifierWatcher", Watcher())
    await bus.request_name("org.kde.StatusNotifierWatcher")
    await bus.request_name("org.kde.StatusNotifierHost-1")
    await asyncio.get_running_loop().create_future()


def labels(node, out):
    _id, props, children = node
    if "label" in props:
        out[props["label"].value.replace("_", "")] = _id
    for c in children:
        labels(c.value, out)
    return out


async def click(label):
    bus = await MessageBus(bus_type=BusType.SESSION).connect()
    service = open(STATE).read().split()[-1]
    path = "/StatusNotifierItem"
    if "/" in service:
        service, path = service.split("/", 1)[0], "/" + service.split("/", 1)[1]
    intro = await bus.introspect(service, path)
    item = bus.get_proxy_object(service, path, intro).get_interface("org.kde.StatusNotifierItem")
    menu_path = await item.get_menu()
    mintro = await bus.introspect(service, menu_path)
    menu = bus.get_proxy_object(service, menu_path, mintro).get_interface("com.canonical.dbusmenu")
    _rev, layout = await menu.call_get_layout(0, -1, [])
    ids = labels(layout, {})
    if label not in ids:
        sys.exit(f"no menu item {label!r}; have {list(ids)}")
    await menu.call_event(ids[label], "clicked", Variant("i", 0), 0)


if __name__ == "__main__":
    if sys.argv[1] == "serve":
        if os.path.exists(STATE):
            os.unlink(STATE)
        asyncio.run(serve())
    else:
        asyncio.run(click(sys.argv[2]))

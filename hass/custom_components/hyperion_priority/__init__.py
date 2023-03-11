#imports
import json
import socket
import ast

DOMAIN = 'hyperion_priority'

from homeassistant.const import CONF_HOST, CONF_PORT

EFFECT = 'effect'
PRIORITY = 'priority'
COLOR = 'color'
ARGS = 'args'
STATE = 'state'

def setup(hass, config):
    """Service to send commands to hyperion."""
    _host = config[DOMAIN][CONF_HOST]
    _port = config[DOMAIN][CONF_PORT]

    def apply_effect(call):
        effect = call.data.get(EFFECT)
        priority = call.data.get(PRIORITY)
        args = call.data.get(ARGS, {})

        if isinstance(args, str):
            args = ast.literal_eval(args)

        json_request(
            {
                "command": "effect",
                "priority": int(priority),
                "effect": {"name": effect, "args": args,}
            }
        )

    def update_tv_state(call):
        state = call.data.get(STATE).lower() == 'on'

        json_request(
            {
                "command": "componentstate",
                "componentstate":{
                    "component":"V4L",
                    "state": state
                }
            }
        )

    def clear_priority(call):
        priority = call.data.get(PRIORITY)
        data = {
                "command": "clear",
                "priority": int(priority),
            }
        json_request(data)
        json_request(data)

    def json_request(request):
        """Communicate with the JSON server."""
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(5)

        try:
            sock.connect((_host, _port))
        except OSError:
            sock.close()
            return False

        sock.send(bytearray(f"{json.dumps(request)}\n", "utf-8"))
        try:
            buf = sock.recv(4096)
        except socket.timeout:
            # Something is wrong, assume it's offline
            sock.close()
            return False

        # Read until a newline or timeout
        buffering = True
        while buffering:
            if "\n" in str(buf, "utf-8"):
                response = str(buf, "utf-8").split("\n")[0]
                buffering = False
            else:
                try:
                    more = sock.recv(4096)
                except socket.timeout:
                    more = None
                if not more:
                    buffering = False
                    response = str(buf, "utf-8")
                else:
                    buf += more

        sock.close()
        return json.loads(response)

    hass.services.register(DOMAIN, 'apply_effect', apply_effect)
    hass.services.register(DOMAIN, 'clear_priority', clear_priority)
    return True

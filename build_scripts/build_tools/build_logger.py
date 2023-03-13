import colorama
import logging
import sys

__all__ = ["log", "color_string", "BuildLogFormatter", "init_logger"]

log = logging.getLogger()

colorama_available = False
try:
    import colorama

    colorama_available = True
    if sys.platform == "win32":
        colorama.just_fix_windows_console()
except:
    print("Running without console coloring, since 'colorama' is not available")


def color_string(
    string: str,
    color: colorama.ansi.AnsiFore
    | colorama.ansi.AnsiBack
    | colorama.ansi.AnsiStyle
    | str,
):
    return color + string + colorama.Style.RESET_ALL


class BuildLogFormatter(logging.Formatter):
    def format(self, record):
        msg_fmt = "%(levelname)s: %(message)s"
        if colorama_available:
            match record.levelno:
                case logging.DEBUG:
                    msg_fmt = color_string(msg_fmt, colorama.Fore.LIGHTBLACK_EX)
                case logging.INFO:
                    msg_fmt = color_string(
                        msg_fmt, colorama.Fore.WHITE + colorama.Style.BRIGHT
                    )
                case logging.WARN:
                    msg_fmt = color_string(msg_fmt, colorama.Fore.YELLOW)
                case logging.ERROR:
                    msg_fmt = color_string(msg_fmt, colorama.Fore.RED)
                case logging.CRITICAL:
                    msg_fmt = color_string(
                        msg_fmt,
                        colorama.Fore.RED
                        + colorama.Back.LIGHTYELLOW_EX
                        + colorama.Style.BRIGHT,
                    )
                case _:
                    self._style._fmt = "%(levelname)s: %(message)s"

        self._style._fmt = msg_fmt
        return super().format(record)


def init_logger(logging_level: int = logging.WARNING):
    handler = logging.StreamHandler()
    handler.setFormatter(BuildLogFormatter())
    log.setLevel(logging_level)
    log.addHandler(handler)

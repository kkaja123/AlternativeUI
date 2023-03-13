import json
import pathlib

from .build_logger import log
from .metadata_reader import VersionInfo


def update_version_string(
    swinfo_path: pathlib.Path | str, new_version_string: VersionInfo
):
    if isinstance(swinfo_path, str):
        swinfo_file = pathlib.Path(swinfo_path)
    else:
        swinfo_file = swinfo_path

    if not swinfo_path.is_file():
        log.error(
            f"The swinfo.json file does not exist at the provided path: {str(swinfo_file)}"
        )
        raise FileNotFoundError

    with open(swinfo_file, mode="r+") as f:
        swinfo_data = json.load(f)
        swinfo_data["version"] = str(new_version_string)
        f.seek(0)
        json.dump(swinfo_data, f, indent=2)
        f.truncate()

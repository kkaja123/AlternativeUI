import json
import os
import pathlib

from .build_logger import log


__all__ = [
    "MetadataReaderError",
    "VersionInfo",
    "get_metadata_dict",
    "make_version_info",
]

# To prevent reading trying to read too much data, set a low limit
METADATA_FILE_SIZE_SAFE_LIMIT_BYTES = 200


class MetadataReaderError(Exception):
    pass


class VersionInfo:
    major: int = 0
    minor: int = 0
    patch: int = 0

    def __init__(self, version_string: str) -> None:
        self.from_version_string(version_string)

    def from_version_digits(self, major: int, minor: int, patch: int) -> None:
        self.major = major
        self.minor = minor
        self.patch = patch

    def from_version_string(self, version_string: str):
        parts = version_string.split(".")
        if len(parts) > 3:
            log.error("Invalid version number string: too many parts found")
            return

        part_index = 0
        for part in parts:
            if len(part) == 0:
                if len(parts) == 1:
                    # Version string is empty.
                    self.reset()
                    return
                else:
                    # Found an empty part in the string. This is an invalid version string.
                    log.error(
                        f"Invalid version number string: a version string part is empty at index = {part_index}"
                    )
                    self.reset()
                    return
            elif not part.isdigit():
                log.error(
                    f"Invalid version number string: a version string part is not a valid digit = {part}"
                )
                self.reset()
                return
            else:
                match part_index:
                    case 0:
                        self.major = int(part)
                    case 1:
                        self.minor = int(part)
                    case 2:
                        self.patch = int(part)
                    case _:
                        break

                part_index += 1

    def get_version_string(self):
        return f"{self.major}.{self.minor}.{self.patch}"

    def __str__(self):
        return self.get_version_string()

    def reset(self):
        self.major = 0
        self.minor = 0
        self.patch = 0


def get_metadata_dict(file: pathlib.Path | str):
    filepath: pathlib.Path
    if isinstance(file, str):
        filepath = pathlib.Path(file)
    else:
        filepath = file

    if not filepath.is_file():
        log.error(f"Metadata file does not exist: {str(filepath)}")
        raise FileNotFoundError

    file_size = filepath.stat().st_size
    FILE_SIZE_WARNING_THRESHOLD = METADATA_FILE_SIZE_SAFE_LIMIT_BYTES - 40
    if file_size > METADATA_FILE_SIZE_SAFE_LIMIT_BYTES:
        log.error(
            f"Metadata file size is past the safety limit. size (bytes) = {file_size}"
        )
        raise MetadataReaderError(
            "Metadata file is unexpectedly large. Possibly unsafe to decode."
        )
    elif file_size > FILE_SIZE_WARNING_THRESHOLD:
        log.warning(
            f"The metadata file size is approaching the safety limit."
            " Consider increasing the limit if the metadata file is genuine."
            " Refer to 'metadata_reader.METADATA_FILE_SIZE_SAFE_LIMIT_BYTES'"
        )

    with open(filepath, mode="r") as f:
        metadata = json.load(f)
    return metadata


def make_version_info(metadata: dict):
    if not "version" in metadata:
        log.error(
            f"The Metadata file does not contain a version string to parse: {metadata}"
        )
        raise KeyError(
            f"The Metadata file does not contain a version string to parse: {metadata}"
        )
    return VersionInfo(metadata["version"])

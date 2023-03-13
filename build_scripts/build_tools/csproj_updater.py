from xml.etree.ElementTree import ElementTree
import defusedxml.ElementTree
import pathlib

from .build_logger import log
from .metadata_reader import VersionInfo


def update_version_string(
    csproj_path: pathlib.Path | str, new_version_string: VersionInfo
):
    if isinstance(csproj_path, str):
        csproj_file = pathlib.Path(csproj_path)
    else:
        csproj_file = csproj_path

    if not csproj_path.is_file():
        log.error(
            f"The .csproj file does not exist at the provided path: {str(csproj_file)}"
        )
        raise FileNotFoundError

    # Pretend that the type of this tree is the standard library ElementTree, even though we know it's really not.
    xml_tree: ElementTree = defusedxml.ElementTree.parse(csproj_file)
    version_element = xml_tree.find("PropertyGroup/Version")
    if version_element is None:
        log.error("Failed to find the '<Version>' element in the .csproj file.")
        raise LookupError

    version_element.text = str(new_version_string)
    xml_tree.write(csproj_file, xml_declaration=False)

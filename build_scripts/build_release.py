import logging
import pathlib
import subprocess
import sys
import zipfile

from build_tools.build_logger import *
from build_tools.build_parameters import (
    BuildParameters,
    make_build_parameters_from_argparse,
)
from build_tools.metadata_reader import (
    VersionInfo,
    get_metadata_dict,
    make_version_info,
)


PROJECT_TITLE = "alternative_ui"  # This build script assumes that the project title is used for the folder name containing a .csproj file with a matching file name.
PROJECT_SHORT_NAME = "AUI"
THIS_SCRIPT_DIR = pathlib.Path(__file__).parent
PROJECT_ROOT_DIR = THIS_SCRIPT_DIR.parent
PROJECT_METADATA = get_metadata_dict(PROJECT_ROOT_DIR / "project_metadata.json")
PROJECT_VERSION_INFO = make_version_info(PROJECT_METADATA)
AUI_VS_PROJECT_DIR = PROJECT_ROOT_DIR / PROJECT_TITLE
MOD_FILES_DIR = PROJECT_ROOT_DIR / "mod_files"
ARCHIVE_PATH_PREFIX = pathlib.Path("BepInEx") / "plugins" / PROJECT_SHORT_NAME
BUILD_OUTPUT_DIR = PROJECT_ROOT_DIR / "build"

init_logger(logging.INFO)


def run_dotnet(configuration: str):
    project_file_path = AUI_VS_PROJECT_DIR / f"{PROJECT_TITLE}.csproj"
    if not project_file_path.is_file():
        # Use root dir as a backup and hopefully dotnet can find our project
        project_file_path = PROJECT_ROOT_DIR

    dotnet_command = ["dotnet", "build", str(project_file_path)]
    result = subprocess.run(dotnet_command)

    if result.returncode != 0:
        log.error("dotnet execution failed")

    return result.returncode


def package(configuration: str):
    if not BUILD_OUTPUT_DIR.exists():
        BUILD_OUTPUT_DIR.mkdir(parents=True)
    elif BUILD_OUTPUT_DIR.is_file():
        log.error(
            f"The build output path currently exists as a file instead of a directory: {str(BUILD_OUTPUT_DIR)}"
        )
        return 1

    package_file_name = (
        f"{PROJECT_TITLE}-{configuration.lower()}-{PROJECT_VERSION_INFO}.zip"
    )

    mod_files_gen = MOD_FILES_DIR.glob("**/*")
    mod_files: list[pathlib.Path] = [file for file in mod_files_gen if file.is_file()]
    mod_files_relative_names = [file.relative_to(MOD_FILES_DIR) for file in mod_files]

    log.debug(f"globbed {len(mod_files)} files in the mod_files dir")
    for modfile in mod_files_relative_names:
        log.debug(f"  {str(modfile)}")

    if len(mod_files) == 0:
        log.error(
            "Failed to discover the mod files in 'mod_files'!"
            " There should be at least swinfo.json in there."
            " Double-check that the repository didn't get munged."
        )

    with zipfile.ZipFile(
        BUILD_OUTPUT_DIR / package_file_name, mode="w", compression=zipfile.ZIP_DEFLATED
    ) as package_zip:
        log.info(f"Creating {package_file_name}")
        package_zip.write(
            AUI_VS_PROJECT_DIR / "bin" / configuration / f"{PROJECT_SHORT_NAME}.dll",
            arcname=ARCHIVE_PATH_PREFIX
            / f"{PROJECT_SHORT_NAME}.dll",  # Put it at the archive's root
        )
        for file_index in range(len(mod_files)):
            package_zip.write(
                mod_files[file_index],
                arcname=ARCHIVE_PATH_PREFIX / mod_files_relative_names[file_index],
            )
        log.info(f"Generated the zip archive with the following contents:")
        package_zip.printdir()

    return 0


def build(parameters: BuildParameters) -> int:
    log.info(f"Project version number: {PROJECT_VERSION_INFO}")

    if not hasattr(parameters, "configurations"):
        log.error(f"Invalid build parameters object passed to build(): {parameters}")
        return 1

    log.info(f"Configurations to build: {parameters.configurations}")

    build_has_failures = False

    for build_config in parameters.configurations:
        log.info(f"Building '{PROJECT_TITLE}' in the '{build_config}' configuration")
        return_code = run_dotnet(build_config)
        if return_code != 0:
            log.error(f"Build FAILURE ❌\n    {PROJECT_TITLE} - {build_config}")
            build_has_failures = True
        else:
            log.info(f"Build SUCCESS ✅\n    {PROJECT_TITLE} - {build_config}")
            package(build_config)

    if build_has_failures:
        return 2
    else:
        return 0


def main() -> int:
    return_code = build(make_build_parameters_from_argparse())
    if return_code != 0:
        log.error("The build failed")
    else:
        success_string = f"Check '{str(BUILD_OUTPUT_DIR)}' for the generated files!!"
        try:
            import colorama

            success_string = color_string(
                success_string, colorama.Fore.CYAN + colorama.Style.BRIGHT
            )
        finally:
            print(success_string)
    return 0


if __name__ == "__main__":
    sys.exit(main())

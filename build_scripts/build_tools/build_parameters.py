import argparse
from collections.abc import Iterable
from .build_logger import *

__all__ = ["BuildParameters"]


def parse_args():
    parser = argparse.ArgumentParser()

    parser.add_argument(
        "--configuration",
        "-c",
        help="The configuration to build",
        type=str.lower,
        choices=["debug", "release", "all"],
        default="release",
    )

    parser.add_argument(
        "--deploy_locally",
        "-d",
        help="If this flag is present, the generated package will be deployed to your KSP 2 game folder (see local_deploy.bat; requires setup). This option is incompatible with the options --configuration=all",
    )

    return parser.parse_args()


class BuildParameters:
    configurations: Iterable[str] = {"Release"}
    deploy_package_locally: bool = False

    def __init__(self, arguments) -> None:
        self.set_values_from_argparse(arguments)

    def set_values_from_argparse(self, arguments):
        if hasattr(arguments, "configuration") and isinstance(
            arguments.configuration, str
        ):
            match str.lower(arguments.configuration):
                case "debug":
                    self.configurations = {"Debug"}
                case "release":
                    self.configurations = {"Release"}
                case "all":
                    self.configurations = {"Debug", "Release"}
                case _:
                    self.configurations = {"Release"}
        else:
            log.warning(
                f"The parsed arguments provided to BuildParameters does not have a valid 'configuration' argument. {arguments}"
            )

        if hasattr(arguments, "deploy_locally") and isinstance(
            arguments.deploy_locally, bool
        ):
            self.deploy_package_locally = arguments.deploy_locally


def make_build_parameters_from_argparse():
    return BuildParameters(parse_args())

{
  description = "A basic flake with a shell";
  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-25.05";
  inputs.flake-utils.url = "github:numtide/flake-utils";

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = nixpkgs.legacyPackages.${system};
      in
      {
        devShell = pkgs.mkShell {
          nativeBuildInputs = with pkgs; [
            bashInteractive
            # mono
            # msbuild
            dotnet-sdk
            dotnetPackages.Nuget
            doxygen
            texlivePackages.epstopdf # needed for doxygen
          ];
          buildInputs = [ ];

          shellHook = ''
            doxygen docs/Doxygen/SteDoxygen
          '';
        };
      });
}

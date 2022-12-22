{
  description = "my project description";

  inputs.flake-utils.url = "github:numtide/flake-utils";

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem
      (system:
        let pkgs = nixpkgs.legacyPackages.${system}; in
        {
          devShells.default = with pkgs; mkShell {
	    packages = [
	      (with dotnetCorePackages; combinePackages [
	        sdk_3_1
		sdk_6_0
	      ])
	      omnisharp-roslyn
	    ];
	  };
        }
      );
}

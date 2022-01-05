# VoltRpc Docfx Project Files

This directory contains the required doc and docfx related files to build the docs website.

Please note, the template that we use is added as a submodule, if you did not clone this repo recursively then you will need to init and update submodules with the commands:

```
git submodule init
git submodule update
```

## Building Docs

Run the following commands in the root of the `docs/` directory.

```
docfx metadata
docfx build
```

Please note: There are some warnings relating to invalid file links to the license and changelog files. This is due to how we include these files. They are safe to ignore.

Yours docs website should now be located `_site/`.
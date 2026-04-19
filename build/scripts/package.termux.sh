#!/usr/bin/env bash

set -e
set -o
set -u
set pipefail

arch=
case "$RUNTIME" in
    termux-arm64|linux-arm64)
        arch=arm64;;
    *)
        echo "Unknown runtime $RUNTIME"
        exit 1;;
esac

cd build

rm -f SourceGit/*.dbg
rm -f SourceGit/*.pdb

mkdir -p resources/deb/opt/sourcegit/
mkdir -p resources/deb/usr/bin
mkdir -p resources/deb/usr/share/applications
mkdir -p resources/deb/usr/share/icons
cp -f SourceGit/* resources/deb/opt/sourcegit
ln -rsf resources/deb/opt/sourcegit/sourcegit resources/deb/usr/bin
cp -r resources/_common/applications resources/deb/usr/share
cp -r resources/_common/icons resources/deb/usr/share
# Calculate installed size in KB
installed_size=$(du -sk resources/deb | cut -f1)
# Update the control file
sed -i -e "s/^Version:.*/Version: $VERSION/" \
    -e "s/^Architecture:.*/Architecture: $arch/" \
    -e "s/^Installed-Size:.*/Installed-Size: $installed_size/" \
    resources/deb/DEBIAN/control
# Build deb package with gzip compression
dpkg-deb -Zgzip --root-owner-group --build resources/deb "sourcegit_$VERSION-1_$arch.deb"

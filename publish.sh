#!/usr/bin/env bash
#
# Publish this project's container image(s) to Docker Hub.
#
# Replaces the Windows build scripts that were run from the original build
# machine (build.bat / BuildCommands.ps1), which is no longer accessible.
#
# Environment overrides:
#   NAMESPACE   Docker Hub namespace           (default: klokedm)
#   TAG         image tag                      (default: latest)
#   PUSH        set to 0 to build without push (default: 1)
#
# Requires: docker login -u "$NAMESPACE"
#
set -euo pipefail

NAMESPACE="${NAMESPACE:-klokedm}"
TAG="${TAG:-latest}"
PUSH="${PUSH:-1}"

cd "$(dirname "${BASH_SOURCE[0]}")"

build_and_push() {
  local image="$1"; shift
  echo "==> building ${image}"
  docker build -t "${image}" "$@"
  if [[ "${PUSH}" == "1" ]]; then
    echo "==> pushing ${image}"
    docker push "${image}"
  else
    echo "==> PUSH=0 set, skipping push of ${image}"
  fi
}

# NOTE: these two images share the private klokedm/vast-private repository and are
# distinguished by TAG, so the TAG override is deliberately not applied here.
build_and_push "${NAMESPACE}/vast-private:ontology-api-image"      -f Dockerfile-api .
build_and_push "${NAMESPACE}/vast-private:ontology-frontend-image" -f Dockerfile-frontend .

# Dockerfile-importer also exists (builds VAST.Annotation.ProxyTest.dll). Its original
# published tag was never recorded, so it is built only on request:
#   BUILD_IMPORTER=1 ./publish.sh
if [[ "${BUILD_IMPORTER:-0}" == "1" ]]; then
  build_and_push "${NAMESPACE}/vast-private:ontology-importer-image" -f Dockerfile-importer .
fi

echo "done: ontology-api-image, ontology-frontend-image"

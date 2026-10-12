#!/usr/bin/env bash
# Applies a misread patch and checks what the README claims about it: the agent's own test passes,
# and every test named as a catcher fails. The patch is reverted on exit.
#
# Usage: .github/misread.sh <patch> <agent's test> <catcher>...
set -euo pipefail

patch=$1 agent=$2
shift 2

# `dotnet test` exits 0 when a filter matches nothing, so the summary line is checked instead.
run() { dotnet test --configuration Release --no-build --filter "FullyQualifiedName~$1" 2>&1 || true; }

git apply "$patch"
trap 'git apply -R "$patch"' EXIT
dotnet build --configuration Release -warnaserror -nologo -v q

if ! run "$agent" | grep -qE '^Passed!.*Passed: +[1-9]'; then
    echo "::error::The agent's own test should pass against the misread: $agent"
    exit 1
fi
echo "Passes against the misread: $agent"

for catcher in "$@"; do
    if ! run "$catcher" | grep -qE '^Failed!.*Failed: +[1-9]'; then
        echo "::error::This test should fail against the misread: $catcher"
        exit 1
    fi
    echo "Catches the misread: $catcher"
done

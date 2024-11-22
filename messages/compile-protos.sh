#!/bin/bash
current_dir=$(pwd)

protos_dir="./"
compiled_dir="../generated/messages"

rm -rf "${current_dir}/${compiled_dir}/*"

files=""
for d in $protos_dir/* ; do
    files="${files} -f ${d##*/} "
done

export MSYS_NO_PATHCONV=1 && docker run \
    -v "${current_dir}/${compiled_dir}":/out \
    -v "${current_dir}/${protos_dir}":/defs \
    namely/protoc-all \
    ${files} \
    -o "/out" \
    -l csharp
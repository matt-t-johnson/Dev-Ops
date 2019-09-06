#!/bin/bash
env=$1
pfx_password=$2

print_script_usage ()
{
  echo "Usage: $0 <env> optional:<password for pfx file>"
  echo 'Usage: $0 dev "0jWZDv(c0EIjAk7o"'
}

if [[ -z "$env" ]]; then
    print_script_usage
    exit 1
fi

# Uppercase environment for commonName
ENV=`echo $env | tr "[:lower:]" "[:upper:]"`

echo "Creating new Certificate Authority (CA) for $ENV environment"
sh create-ca.sh $env

echo "Creating new client certificate for $ENV environment"
sh create-client-cert.sh $env

echo "Create PFX file for $ENV environment"
sh create-pfx.sh ../output/${env}-client.key ../output/${env}-client.crt ../output/foo-${env}.pfx $pfx_password

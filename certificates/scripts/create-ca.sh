#!/bin/bash
env=$1
keyPath=$2

print_script_usage ()
{
  echo "Usage: $0 <env>"
  echo "Usage: $0 dev"

  echo "Optional Argument: <path to existing private key>"
  echo "Usage: $0 dev ~/ca_private.key"
}

if [[ -z "$env" ]]; then
    print_script_usage
    exit 1
fi

# Uppercase environment for commonName
ENV=`echo $env | tr "[:lower:]" "[:upper:]"`

# Create an environment specific openssl config based on openssl.conf
sed "s/\ENV_PLACEHOLDER/$ENV/" ../openssl.conf > ../output/${env}.openssl.conf

# TODO output results
if [[ -z "$keyPath" ]]; then
  # Generate a self signed Certificate Authority with a new key:
  echo "Creating CA Certificate for $ENV environment with new key"
  openssl req -new -x509 -sha256 -newkey rsa:4096 -nodes -keyout ../output/${env}-ca.key -config ../output/${env}.openssl.conf -extensions v3_ca -days 730 -out ../output/${env}-ca.crt

  else
    # Generate a self signed certificate for the CA with existing key:
    echo "Creating CA Certificate for $ENV environment with existing key $keyPath"
    openssl req -new -x509 -key $keyPath -config ../output/openssl.${ENV}.conf -extensions v3_ca -days 730 -out ../output/${env}-ca.crt
fi

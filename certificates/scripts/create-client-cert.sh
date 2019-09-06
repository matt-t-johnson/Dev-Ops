#!/bin/bash
env=$1

print_script_usage ()
{
  echo "Usage: $0 <env>"
  echo "Usage: $0 dev"
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
# Create a Certificate Signing Request (CSR)
echo "Creating a Certificate Signing Request (CSR) for $ENV"
openssl req -newkey rsa:2048 -nodes -keyout ../output/${env}-client.key -config ../output/${env}.openssl.conf -out ../output/${env}-client.csr

# Sign the CSR using the CA
echo "Signing the client certificate for $ENV"
openssl x509 -req -sha256 -days 730 -in ../output/${env}-client.csr -CA ../output/${env}-ca.crt -CAkey ../output/${env}-ca.key -CAcreateserial -out ../output/${env}-client.crt

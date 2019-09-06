#!/bin/bash
keyPath=$1
certPath=$2
outputPath=$3
password=$4

print_script_usage ()
{
  echo "Usage: $0 <private key> <certificate> <output location> optional:<password>"
  echo "Usage: $0 ../DEV/dev-client.key ../DEV/dev-client.crt ../foo.pfx"
  echo "Usage: $0 ../DEV/dev-client.key ../DEV/dev-client.crt foo.pfx 'p@ssWerd' "
}

if [[ -z "$keyPath" ]]; then
    print_script_usage
    exit 1
fi

if [[ -z "$certPath" ]]; then
    print_script_usage
    exit 1
fi

if [[ -z "$outputPath" ]]; then
    print_script_usage
    exit 1
fi

# Generate a PFX file containing the client certificate and private key
openssl pkcs12 -export -out $outputPath -inkey $keyPath -in $certPath -passout pass:$password

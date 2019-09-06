Create a Certificate Authority (CA)
1. Create a private key
	openssl genrsa -out ca.key 4096

2. Generate a self signed certificate for the CA with existing key:
(Note, must use config file)
openssl req -new -x509 -sha256 -key ca.key -config openssl.conf -extensions v3_ca -days 730 -out ca.crt

Generate a self signed Certificate Authority with a new key:
openssl req -new -x509 -sha256 -newkey rsa:4096 -nodes -keyout ca.key -config openssl.conf -extensions v3_ca -days 730 -out ca.crt


Create a Certificate Signed by the Internal CA
1. Create a Certificate Signing Request (CSR)
openssl req -newkey rsa:2048 -nodes -keyout foo.key -config openssl.conf -out foo.csr

2. Sign the CSR using the CA
openssl x509 -req -sha256 -days 730 -in foo.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out foo.crt


Generate a PFX file containing the client certificate and private key
openssl pkcs12 -export -out foo.pfx -inkey foo.key -in foo.crt -passout pass:


Verify a cert is issued by a CA
openssl verify -verbose -CAfile ca.crt  foo.crt


NOTE
Whatever method you use to generate the certificate and key files, the Common Name value used for the server and client certificates/keys must each differ from the Common Name value used for the CA certificate. Otherwise, the certificate and key files will not work for servers compiled using OpenSSL.

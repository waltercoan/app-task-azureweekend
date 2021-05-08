# Azure Weekend 08/05/2021 - Utilizando Azure Batch em Linux

## Configurações
1) Registrar uma aplicação no Azure Active Directory
2) Criar as seguintes variáveis de ambiente, contendo as chaves de uma Aplicação registrada no Azure Active Directory
AZURE_CLIENT_ID (obter valor na tela iniciado da aplicação registrada no AAD )
AZURE_CLIENT_SECRET (obter no registro da aplicação registrada no AAD, na opção certificados e segredos)
AZURE_TENANT_ID (obter valor na tela iniciado da aplicação registrada no AAD )
3) Criar uma conta de armazenamento, dentro dela um container e neste container deve haver uma regra no IAM dando permissão a aplicação registrada no AAD para ser Storage Blob Data Contributor.
4) Modificar o nome da conta de armazenamento dentro do código fonte da classe Service.cs

## Build
docker build -t apptaskazureweekend:latest .

docker tag apptaskazureweekend:latest nome.azurecr.io/apptaskazureweekend:latest

docker login nome.azurecr.io

docker push nome.azurecr.io/apptaskazureweekend:latest


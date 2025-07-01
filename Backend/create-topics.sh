#!/bin/bash

# Nome do container conforme definido no docker-compose.yml
CONTAINER_NAME="kafka"

# Lista de tópicos que você deseja criar
TOPICS=("ativos.subscricao" "precos.atualizados")

# Configurações padrão
PARTITIONS=1
REPLICATION_FACTOR=1
BOOTSTRAP_SERVER="kafka:9092"

echo "Criando tópicos Kafka dentro do container $CONTAINER_NAME..."

for TOPIC in "${TOPICS[@]}"
do
  echo "➤ Criando tópico: $TOPIC"
  docker exec "$CONTAINER_NAME" kafka-topics.sh \
    --create \
    --if-not-exists \
    --topic "$TOPIC" \
    --partitions $PARTITIONS \
    --replication-factor $REPLICATION_FACTOR \
    --bootstrap-server "$BOOTSTRAP_SERVER"
done

echo "✔ Todos os tópicos foram criados (se ainda não existiam)."

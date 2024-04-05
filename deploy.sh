# Check for Docker
if ! command -v docker &> /dev/null
then
	echo "Docker not found. Installing..."
	sudo apt-get update && sudo apt-get install docker.io -y
fi

# Check for Docker Compose
DOCKER_COMPOSE_VERSION=$(curl -s https://api.github.com/repos/docker/compose/releases/latest | grep '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')
if ! command -v docker-compose &> /dev/null
then
	echo "Docker Compose not found. Installing..."
	if ! command -v curl &> /dev/null; then
    		echo "Curl not found. Installing..."
		sudo apt-get update && sudo apt-get install -y curl
	fi
	sudo curl -L "https://github.com/docker/compose/releases/download/${DOCKER_COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
	sudo chmod +x /usr/local/bin/docker-compose
fi

echo "Building and running Docker containers..."
docker-compose up --build -d

echo "Setup complete. RaffleKing running at http://localhost:8001/"
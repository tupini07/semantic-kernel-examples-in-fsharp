echo 'export PATH="$HOME/.local/bin/:${PATH}"' >> ~/.zshrc
echo 'export PATH="$HOME/.dotnet/:${PATH}"' >> ~/.zshrc

wget https://dot.net/v1/dotnet-install.sh && \
    mkdir -p ~/.local/bin/ && \
    chmod +x dotnet-install.sh && \
    mv dotnet-install.sh ~/.local/bin/ && \
    ~/.local/bin/dotnet-install.sh --channel 7.0

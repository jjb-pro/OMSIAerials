# OMSI Aerials

**OMSI Aerials** is a lightweight local tile server that provides aerial and map overlays for OMSI 2. Designed for easy setup and customization, it allows you to use modern map providers like Bing, Google, Mapbox, and OpenStreetMap right inside the OMSI editor.

> [!WARNING]  
> **Beta Notice:** OMSI Aerials is currently in early development. Bing support is stable; more features and providers will be added soon.

## Features

- **Supports multiple map providers** – Bing, Google, Mapbox, OpenStreetMap  
- **Choose between aerial (satellite) and map overlays**  
- **Built-in link wizard** – Easily generate the correct link for your `options.cfg` file.  
- **Runs locally on your machine** – No internet hosting required.  
- **Lightweight executable** – No installer, just unzip and run.  
- **Simple integration with OMSI 2**  
- **Actively Maintained** – Bug fixes, features, and improvements are continuously being worked on.  

## Getting Started

Since there are no official releases yet, you’ll need to compile OMSI Aerials manually:

1. **Clone the repository**:

    ```bash
    git clone https://github.com/jjb-pro/OMSIAerials.git
    ```

2. **Open in Visual Studio**:
   - Use **Visual Studio 2022 or later**  
   - Open the `.sln` file  
   - Build the application  

3. **Run the server** on your desired device.  
A black console window will appear — this means the server is running.

4. **Configure OMSI 2**  
Add the following line to your `options.cfg` file in the OMSI Directory:

    ```
    http://localhost:7005/aerial?provider={yourProvider}&x=~x&y=~y&z=~z&tileset=Aerial&apiCode={yourApiKey}
    ```

> 🔑 Replace `{yourProvider}` and `{yourApiKey}` with the actual values.

**OR:** Use the wizard by opening `http://localhost:7005/` to create the URL.

## Contributing

Contributions, feedback, and suggestions are welcome!  
Whether you’re fixing a bug, requesting a feature, or improving documentation — feel free to reach out or fork and improve.

## License

[MIT License](LICENSE)

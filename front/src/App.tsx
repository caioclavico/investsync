import React from "react";
import "./App.css";
import logo from "./assets/logos/investsync-dark.png";

function App() {
    return (
        <div
            className="App"
            style={{
                minHeight: "100vh",
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
            }}
        >
            <img
                src={logo}
                alt="Logo InvestSync"
                style={{ width: 300, marginBottom: 24 }}
            />
            <p>Welcome to InvestSync, your personal investment assistant.</p>
            <span
                style={{
                    color: "#888",
                    marginTop: 16,
                    fontStyle: "italic",
                }}
            >
                Página em desenvolvimento
            </span>
        </div>
    );
}

export default App;

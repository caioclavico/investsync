import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { AuthService, UserResponse } from "../../services/authService";
import logo from "../../assets/logos/investsync-dark.png";
import "./Dashboard.css";

const Dashboard: React.FC = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState<UserResponse | null>(null);

  useEffect(() => {
    const userData = AuthService.getUserData();
    if (userData) {
      setUser(userData);
    } else {
      // Se não há dados do usuário, redirecionar para login
      navigate("/login");
    }
  }, [navigate]);

  const handleLogout = () => {
    AuthService.logout();
    toast.success("Logout realizado com sucesso!");
    navigate("/login");
  };

  if (!user) {
    return (
      <div className="dashboard-loading">
        <p>Carregando...</p>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <div className="header-content">
          <div className="header-left">
            <img src={logo} alt="InvestSync Logo" className="header-logo" />
            <h1>InvestSync</h1>
          </div>
          <div className="header-right">
            <span className="user-name">Olá, {user.name}!</span>
            <button className="logout-button" onClick={handleLogout}>
              Sair
            </button>
          </div>
        </div>
      </header>

      <main className="dashboard-main">
        <div className="dashboard-content">
          <div className="welcome-section">
            <h2>Bem-vindo ao seu painel de investimentos!</h2>
            <p>Aqui você pode acompanhar seus investimentos, realizar transações e muito mais.</p>
          </div>

          <div className="user-info-card">
            <h3>Informações da conta</h3>
            <div className="user-details">
              <div className="detail-item">
                <strong>Nome:</strong> {user.name}
              </div>
              <div className="detail-item">
                <strong>E-mail:</strong> {user.email}
              </div>
              <div className="detail-item">
                <strong>ID:</strong> {user.id}
              </div>
            </div>
          </div>

          <div className="features-grid">
            <div className="feature-card">
              <h4>💰 Carteira</h4>
              <p>Visualize seu saldo e investimentos</p>
              <button className="feature-button">Em desenvolvimento</button>
            </div>

            <div className="feature-card">
              <h4>📈 Transações</h4>
              <p>Compre e venda ações</p>
              <button className="feature-button">Em desenvolvimento</button>
            </div>

            <div className="feature-card">
              <h4>📊 Relatórios</h4>
              <p>Acompanhe o desempenho dos seus investimentos</p>
              <button className="feature-button">Em desenvolvimento</button>
            </div>

            <div className="feature-card">
              <h4>⚙️ Configurações</h4>
              <p>Gerencie sua conta e preferências</p>
              <button className="feature-button">Em desenvolvimento</button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Dashboard;

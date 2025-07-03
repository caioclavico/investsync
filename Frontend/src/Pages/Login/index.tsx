import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { toast } from "react-toastify";
import logo from "../../assets/logos/investsync-dark.png";
import { AuthService, ApiException } from "../../services/authService";
import "./Login.css";

interface LoginForm {
  email: string;
  password: string;
}

const Login: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [formData, setFormData] = useState<LoginForm>({
    email: "",
    password: "",
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  // Verificar se há mensagem de sucesso do cadastro
  useEffect(() => {
    if (location.state?.message) {
      setSuccessMessage(location.state.message);
      if (location.state?.email) {
        setFormData((prev) => ({ ...prev, email: location.state.email }));
      }
    }
  }, [location.state]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    if (error) setError(""); // Limpar erro quando o usuário começar a digitar
    if (successMessage) setSuccessMessage(""); // Limpar mensagem de sucesso
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");
    setSuccessMessage("");

    try {
      // Chamar API de login
      const credentials = {
        email: formData.email.trim(),
        password: formData.password,
      };

      const response = await AuthService.login(credentials); // Salvar token e dados do usuário
      AuthService.saveToken(response.token);
      AuthService.saveUserData(response.user);

      // Sucesso - redirecionar para home/dashboard
      toast.success(`Bem-vindo(a), ${response.user.name}!`);
      navigate("/home");
    } catch (error) {
      if (error instanceof ApiException) {
        // Tratar erros específicos da API
        if (error.status === 401) {
          setError("E-mail ou senha incorretos. Verifique suas credenciais.");
        } else if (error.status === 400) {
          setError("Dados inválidos. Verifique e-mail e senha.");
        } else if (error.status === 0) {
          toast.error("Não foi possível conectar com o servidor. Verifique sua conexão.");
          setError("Não foi possível conectar com o servidor. Verifique sua conexão.");
        } else {
          setError(error.message || "Erro ao fazer login. Tente novamente.");
        }
      } else {
        toast.error("Erro inesperado. Tente novamente mais tarde.");
        setError("Erro inesperado. Tente novamente mais tarde.");
      }

      console.error("Erro no login:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleGoToRegister = () => {
    navigate("/register");
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          <img src={logo} alt="InvestSync Logo" className="login-logo" />
          <h1>Bem-vindo de volta!</h1>
          <p>Entre em sua conta para acessar seus investimentos</p>
        </div>

        <form onSubmit={handleSubmit} className="login-form">
          {successMessage && <div className="success-message">{successMessage}</div>}
          {error && <div className="error-message">{error}</div>}

          <div className="form-group">
            <label htmlFor="email">E-mail</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              placeholder="Digite seu e-mail"
              required
              disabled={isLoading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Senha</label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleInputChange}
              placeholder="Digite sua senha"
              required
              disabled={isLoading}
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="login-button" disabled={isLoading}>
              {isLoading ? "Entrando..." : "Entrar"}
            </button>
          </div>

          <div className="login-footer">
            <p>
              Não tem uma conta?{" "}
              <button type="button" className="link-button" onClick={handleGoToRegister} disabled={isLoading}>
                Cadastre-se aqui
              </button>
            </p>

            <button type="button" className="link-button forgot-password" disabled={isLoading}>
              Esqueceu sua senha?
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Login;

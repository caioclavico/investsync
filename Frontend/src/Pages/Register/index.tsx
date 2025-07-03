import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import logo from "../../assets/logos/investsync-dark.png";
import { AuthService, ApiException } from "../../services/authService";
import "./Register.css";

interface RegisterForm {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

const Register: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<RegisterForm>({
    name: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    if (error) setError(""); // Limpar erro quando o usuário começar a digitar
  };

  const validateForm = (): boolean => {
    if (!formData.name.trim()) {
      setError("Nome é obrigatório");
      return false;
    }

    if (!formData.email.trim()) {
      setError("E-mail é obrigatório");
      return false;
    }

    if (formData.password.length < 6) {
      setError("Senha deve ter pelo menos 6 caracteres");
      return false;
    }

    if (formData.password !== formData.confirmPassword) {
      setError("Senhas não coincidem");
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);

    try {
      // Chamar API de cadastro
      const userData = {
        name: formData.name.trim(),
        email: formData.email.trim(),
        password: formData.password,
      };

      const response = await AuthService.register(userData);

      // Sucesso - redirecionar para login com mensagem
      toast.success(`Cadastro realizado com sucesso! Bem-vindo(a), ${response.name}!`);
      navigate("/login", {
        state: {
          message: "Conta criada com sucesso! Faça seu login.",
          email: formData.email,
        },
      });
    } catch (error) {
      if (error instanceof ApiException) {
        // Tratar erros específicos da API
        if (error.status === 409) {
          setError("Este e-mail já está cadastrado. Tente fazer login.");
        } else if (error.status === 400) {
          setError("Dados inválidos. Verifique as informações e tente novamente.");
        } else if (error.status === 0) {
          toast.error("Não foi possível conectar com o servidor. Verifique sua conexão.");
          setError("Não foi possível conectar com o servidor. Verifique sua conexão.");
        } else {
          setError(error.message || "Erro ao criar conta. Tente novamente.");
        }
      } else {
        toast.error("Erro inesperado. Tente novamente mais tarde.");
        setError("Erro inesperado. Tente novamente mais tarde.");
      }

      console.error("Erro no cadastro:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleGoToLogin = () => {
    navigate("/login");
  };

  return (
    <div className="register-container">
      <div className="register-card">
        <div className="register-header">
          <img src={logo} alt="InvestSync Logo" className="register-logo" />
          <h1>Crie sua conta</h1>
          <p>Comece sua jornada de investimentos hoje mesmo</p>
        </div>

        <form onSubmit={handleSubmit} className="register-form">
          {error && <div className="error-message">{error}</div>}

          <div className="form-group">
            <label htmlFor="name">Nome completo</label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleInputChange}
              placeholder="Digite seu nome completo"
              required
              disabled={isLoading}
            />
          </div>

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
              placeholder="Digite uma senha (mín. 6 caracteres)"
              required
              disabled={isLoading}
              minLength={6}
            />
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Confirmar senha</label>
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleInputChange}
              placeholder="Confirme sua senha"
              required
              disabled={isLoading}
              minLength={6}
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="register-button" disabled={isLoading}>
              {isLoading ? "Criando conta..." : "Criar conta"}
            </button>
          </div>

          <div className="register-footer">
            <p>
              Já tem uma conta?{" "}
              <button type="button" className="link-button" onClick={handleGoToLogin} disabled={isLoading}>
                Faça login aqui
              </button>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Register;

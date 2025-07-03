// Tipos de dados para as requisições/respostas da API
export interface UserRegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface UserLoginRequest {
  email: string;
  password: string;
}

export interface UserResponse {
  id: string;
  name: string;
  email: string;
}

export interface UserLoginResponse {
  token: string;
  user: UserResponse;
}

export interface ApiError {
  message: string;
  status: number;
}

// Configuração da API
// Durante o desenvolvimento, podemos usar HTTP se tivermos problemas com HTTPS
const useHttpInDev = true; // Defina como false para usar HTTPS
const apiUrlFromEnv = process.env.REACT_APP_API_URL || "https://localhost:5001";
const API_BASE_URL = useHttpInDev
  ? apiUrlFromEnv.replace("https://", "http://").replace("5001", "5000")
  : apiUrlFromEnv;

// Classe de erro personalizada para API
export class ApiException extends Error {
  public status: number;

  constructor(message: string, status: number) {
    super(message);
    this.status = status;
    this.name = "ApiException";
  }
}

// Serviço de autenticação
export class AuthService {
  private static async request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;

    const config: RequestInit = {
      headers: {
        "Content-Type": "application/json",
        ...options.headers,
      },
      ...options,
    };

    // Log para depuração
    console.log(`🔄 Fazendo requisição para: ${url}`);
    console.log("Configuração:", config);

    try {
      const response = await fetch(url, config);
      console.log(`✅ Resposta recebida: Status ${response.status}`);

      if (!response.ok) {
        let errorMessage = "Erro na requisição";

        try {
          const errorData = await response.text();
          console.error("Dados do erro:", errorData);
          console.error("Dados do erro:", errorData);
          errorMessage = errorData || `Erro ${response.status}: ${response.statusText}`;
        } catch (parseError) {
          console.error("Erro ao processar resposta de erro:", parseError);
          errorMessage = `Erro ${response.status}: ${response.statusText}`;
        }

        throw new ApiException(errorMessage, response.status);
      }

      try {
        const data = await response.json();
        console.log("Dados recebidos:", data);
        return data;
      } catch (parseError) {
        console.error("Erro ao processar resposta JSON:", parseError);
        throw new ApiException("Erro ao processar resposta do servidor", 500);
      }
    } catch (error) {
      console.error("Erro na requisição:", error);

      if (error instanceof ApiException) {
        throw error;
      }

      // Erro de rede ou conexão
      if (error instanceof TypeError && error.message.includes("fetch")) {
        console.error("Erro de conexão detectado:", error.message);
        throw new ApiException("Erro de conexão. Verifique sua internet ou se a API está rodando.", 0);
      }

      throw new ApiException("Erro inesperado. Tente novamente.", 500);
    }
  }

  // Registrar novo usuário
  static async register(userData: UserRegisterRequest): Promise<UserResponse> {
    return this.request<UserResponse>("/auth/register", {
      method: "POST",
      body: JSON.stringify(userData),
    });
  }

  // Fazer login
  static async login(credentials: UserLoginRequest): Promise<UserLoginResponse> {
    return this.request<UserLoginResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify(credentials),
    });
  }

  // Salvar token no localStorage
  static saveToken(token: string): void {
    localStorage.setItem("authToken", token);
  }

  // Obter token do localStorage
  static getToken(): string | null {
    return localStorage.getItem("authToken");
  }

  // Remover token (logout)
  static removeToken(): void {
    localStorage.removeItem("authToken");
  }

  // Verificar se usuário está logado
  static isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null && token.length > 0;
  }

  // Obter dados do usuário do localStorage
  static getUserData(): UserResponse | null {
    const userData = localStorage.getItem("userData");
    return userData ? JSON.parse(userData) : null;
  }

  // Salvar dados do usuário
  static saveUserData(user: UserResponse): void {
    localStorage.setItem("userData", JSON.stringify(user));
  }

  // Remover dados do usuário
  static removeUserData(): void {
    localStorage.removeItem("userData");
  }

  // Logout completo
  static logout(): void {
    this.removeToken();
    this.removeUserData();
  }
}

/** 用户注册请求 */
declare interface RegisterRequest {
  email: string;
  password: string;
  username: string;

  [key: string]: any;
}

/** 用户注册响应 */
declare interface RegisterResponse {
  userId: number;          // C# Guid → TS string
  email: string;
  username: string;
  createdAt: string;       // ISO-8601 日期串

  [key: string]: any;
}

/** 登录请求 */
declare interface LoginRequest {
  email: string;
  password: string;

  [key: string]: any;
}

/** 登录响应 */
declare interface LoginResponse {
  userId: number;
  email: string;
  username: string;
  token: string;
  tokenExpiry: string;     // ISO-8601 日期串

  [key: string]: any;
}

/** 更新用户名请求 */
declare interface UpdateUsernameRequest {
  newUsername: string;

  [key: string]: any;
}

/** 邮箱验证请求 */
declare interface VerifyEmailRequest {
  verificationToken: string;

  [key: string]: any;
}

/** 重发验证邮件请求 */
declare interface ResendVerificationRequest {
  email: string;

  [key: string]: any;
}

export type  {
  RegisterRequest,
  RegisterResponse,
  LoginRequest,
  LoginResponse,
  UpdateUsernameRequest,
  VerifyEmailRequest,
  ResendVerificationRequest
};

/** 用户注册请求 */
declare interface RegisterRequest {
  email: string;
  password: string;
  username: string;
}

/** 用户注册响应 */
declare interface RegisterResponse {
  userId: string;          // C# Guid → TS string
  email: string;
  username: string;
  createdAt: string;       // ISO-8601 日期串
}

/** 登录请求 */
declare interface LoginRequest {
  email: string;
  password: string;
}

/** 登录响应 */
declare interface LoginResponse {
  userId: string;
  email: string;
  username: string;
  token: string;
  tokenExpiry: string;     // ISO-8601 日期串
}

/** 更新用户名请求 */
declare interface UpdateUsernameRequest {
  newUsername: string;
}

/** 邮箱验证请求 */
declare interface VerifyEmailRequest {
  verificationToken: string;
}

/** 重发验证邮件请求 */
declare interface ResendVerificationRequest {
  email: string;
}

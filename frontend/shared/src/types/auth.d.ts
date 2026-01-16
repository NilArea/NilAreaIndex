export namespace dto {
  export namespace auth {
    /** 用户注册请求 */
    export interface RegisterRequest {
      email: string;
      password: string;
      username: string;
    }

    /** 用户注册响应 */
    export interface RegisterResponse {
      userId: string;          // C# Guid → TS string
      email: string;
      username: string;
      createdAt: string;       // ISO-8601 日期串
    }

    /** 登录请求 */
    export interface LoginRequest {
      email: string;
      password: string;
    }

    /** 登录响应 */
    export interface LoginResponse {
      userId: string;
      email: string;
      username: string;
      token: string;
      tokenExpiry: string;     // ISO-8601 日期串
    }

    /** 更新用户名请求 */
    export interface UpdateUsernameRequest {
      newUsername: string;
    }

    /** 邮箱验证请求 */
    export interface VerifyEmailRequest {
      verificationToken: string;
    }

    /** 重发验证邮件请求 */
    export interface ResendVerificationRequest {
      email: string;
    }
  }
}

/* 便捷类型导出（可选） */
export type RegisterReq = dto.auth.RegisterRequest;
export type RegisterRes = dto.auth.RegisterResponse;
export type LoginReq = dto.auth.LoginRequest;
export type LoginRes = dto.auth.LoginResponse;
export type UpdUsernameReq = dto.auth.UpdateUsernameRequest;
export type VerifyEmailReq = dto.auth.VerifyEmailRequest;
export type ResendVerifyReq = dto.auth.ResendVerificationRequest;

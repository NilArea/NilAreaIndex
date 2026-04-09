using NilArea.Web.Configure;
using Orleans.Dashboard;

var builder = WebApplication.CreateBuilder(args);

#region Builder

// ===== 基础设施 =====
builder
    .ConfigureNilareaOrleans() //核心数据操作
    .ConfigureRedis() //快速简单检索
    .ConfigureOpenSearch(); //快速复杂检索
// ===== 数据验证 =====
builder
    .ConfigureDataValidator();
// ===== 全局异常处理 =====
builder
    .ConfigureGlobalExceptionHandler();
// ===== 安全配置（CORS、认证、授权） =====
builder
    .ConfigureSecurity();
// ===== 控制器 =====
builder.Services
    .AddControllers();

#endregion

var app = builder.Build();

#region Application

// 异常处理
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// HTTPS 重定向
app.UseHttpsRedirection();
// 静态文件（应放在路由之前，避免不必要的路由处理）
app.UseStaticFiles();
// 路由匹配（必须放在身份验证、授权等之前）
app.UseRouting();
// 跨域配置
app.UseCors();
// 身份验证（依赖路由，但必须在授权之前）
app.UseAuthentication();
// 授权
app.UseAuthorization();
// 自定义中间件
//app.UseMiddleware<MyCustomMiddleware>();
// Orleans仪表盘映射
app.MapOrleansDashboard("/orleans");
// API 终结点映射
app.MapControllers();
// SPA 回退路由
app.MapFallbackToFile("index.html");

#endregion

app.Run();
return 0;
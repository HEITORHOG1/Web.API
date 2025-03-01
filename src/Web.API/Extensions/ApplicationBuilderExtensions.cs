using Web.Infrastructure.Data.Seed;

namespace Web.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureMiddleware(this WebApplication app)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            // Adicione configuração de CORS se necessário
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Adicione tratamento para servir arquivos estáticos se necessário
            app.UseStaticFiles();
            app.MapControllers();

            // Inicializando a base de dados com papéis e usuários
            using var scope = app.Services.CreateScope();
            DatabaseInitializer.SeedData(scope.ServiceProvider).Wait();
        }
    }
}
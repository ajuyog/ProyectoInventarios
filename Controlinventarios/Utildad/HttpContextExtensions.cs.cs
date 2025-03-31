using Microsoft.EntityFrameworkCore;

namespace Controlinventarios.Utildad
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            ArgumentNullException.ThrowIfNull(httpContext);
            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Append("cantidadTotalRegistros", cantidad.ToString());
        }

        public async static Task TInsertarParametrosPaginacion<T>(this HttpContext httpContext, IQueryable<T> queryable, int cantidadRegistrosPorPagina)
        {
            double cantidad = await queryable.CountAsync();
            double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina);
            httpContext.Response.Headers.Append("cantidadPaginas", cantidadPaginas.ToString());
        }
    }
}
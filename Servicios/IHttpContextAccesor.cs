namespace ManejoPresupuesto.Servicios
{
    public interface IHttpContextAccesor
    {
        HttpContext HttpContext { get; set; }
    }
}
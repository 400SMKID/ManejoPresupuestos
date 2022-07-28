﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int Id, int usuarioId);
    }


    public class RepositorioCuentas : IRepositorioCuentas
    {

        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance)
                    VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance)

                    SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }


        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"
                                    SELECT Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre AS TipoCuenta
                                    FROM Cuentas
                                    INNER JOIN TiposCuentas tc
                                    ON tc.Id = Cuentas.TipoCuentaId
                                    WHERE tc.UsuarioId = @UsuarioId
                                    ORDER BY tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int Id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                                    @"SELECT Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, tc.Id
                                    FROM Cuentas
                                    INNER JOIN TiposCuentas tc
                                    ON tc.Id = Cuentas.TipoCuentaId
                                    WHERE tc.UsuarioId = @UsuarioId AND Cuentas.Id =@Id", new {Id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
                            SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion,
                            TipoCuentaId = @TipoCuentaId
                            WHERE Id = @Id;", cuenta);
        }

    }
}
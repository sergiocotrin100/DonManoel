using Core.Entities;
using Core.Interfaces;
using Dapper;
using Infrastructure.Context;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class MesaRepository : IMesaRepository
    {
        private readonly IDonConnection _connection;

        public MesaRepository(IDonConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Mesa>> GetAll()
        {
            var mesas = new List<Mesa>();
            for (int i = 0; i < 12; i++)
            {
                string uso = "N";
                if (i % 2 == 0)
                    uso = "S";

                var data = DateTime.Now.AddHours(-i);
                var valor = i * 10.45;

                mesas.Add(new Mesa()
                {
                    Id = i,
                    Nome = "Mesa " + i.ToString(),
                    Numero = i,
                    Uso = uso,
                    DataAberturaPedido = data,
                    ValorPedido= valor
                });
            }
            return mesas;

            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        M.ID,
                        M.NOME,
                        M.NUMERO,
                        (
                            SELECT 'S' FROM DOTNET_PEDIDO P
                            WHERE P.ID_MESA = M.ID_MESA
                            AND ROWNUM <2 
                        ) USO
                    FROM MESAS M
                 ");
                var parametros = new DynamicParameters();
                var model = await conn.QueryAsync<Mesa>(cmd.ToString(), parametros);
                return model.ToList();
            }
        }
    }
}

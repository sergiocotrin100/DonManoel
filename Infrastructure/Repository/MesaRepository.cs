using Core.Entities;
using Core.Interfaces;
using Dapper;
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
            using (IDbConnection conn = _connection.GetConnection())
            {
                var cmd = new StringBuilder();
                cmd.AppendFormat(@"
                    SELECT 
                        M.ID,
                        M.NOME,
                        M.NUMERO,
                        CASE WHEN P.ID IS NOT NULL THEN 'S' ELSE 'N' END USO,
                        P.DATA DATAABERTURAPEDIDO,
                        P.VALOR_ITENS VALORPEDIDO,
                        U.NOME ATENDENTE,
                        P.ID IDPEDIDO,
                        P.TAXA_SERVICO TAXASERVICO,
                        NVL(P.VALOR_ITENS,0) ValorPedido
                    FROM MESAS M
                    LEFT JOIN DOTNET_PEDIDO P ON P.ID_MESA = M.ID AND P.ID_STATUS_PEDIDO NOT IN(6,7)
                    LEFT JOIN PCO_USR U ON U.ID = P.ID_USUARIO
                 ");
                var parametros = new DynamicParameters();
                var model = await conn.QueryAsync<Mesa>(cmd.ToString(), parametros);
                return model.ToList();
            }
        }
    }
}

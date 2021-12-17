using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Entities.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Coupon> GetDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM COUPON WHERE productName = @ProductName",
                    new { ProductName = productName });

                if (coupon == null)
                {
                    return new Coupon
                    {
                        ProductName = "No Discount",
                        Amount = 0,
                        Description = "No Dicount Available",
                        Id = 0
                    };
                }

                return coupon;
            };

        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await connection.ExecuteAsync(
                    "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ( @ProductName, @Description, @Amount)",
                    new
                    {
                        ProductName = coupon.ProductName,
                        Description = coupon.Description,
                        Amount = coupon.Amount
                    });

                return affected > 0;
            }
        }
        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await connection.ExecuteAsync(
                   "UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE ID=@ID",
                   new
                   {
                       ProductName = coupon.ProductName,
                       Description = coupon.Description,
                       Amount = coupon.Amount,
                       ID = coupon.Id
                   });
                return affected > 0;
            }
        }
        public async Task<bool> DeleteDiscount(string productName)
        {
            using (var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
            {
                var affected = await connection.ExecuteAsync(
                      "DELETE FROM Coupon WHERE ProductName=@ProductName",
                      new
                      {
                          ProductName = productName
                      });
                return affected > 0;
            }
        }

    }
}

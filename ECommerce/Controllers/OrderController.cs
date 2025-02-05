using ECommerce.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data;

using System.Data.SqlClient;
using Dapper;  // Use Dapper for SQL queries (Optional)
using ECommerce.Model;
using System.Linq;
using System.Linq.Expressions;
namespace ECommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("getOrderInfo")]
        public IActionResult GetOrderInfo([FromBody] Customers request)
        {
            // Validate input
            if (request == null || string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.CustomerId))
            {
                return BadRequest(new { error = "Invalid input data" });
            }
            try
            {

                // Fetch customer details
                var customer = GetCustomerDetails(request.CustomerId, request.FirstName);

                if (customer == null)
                {
                    return BadRequest(new { error = "Invalid customer or email address" });
                }

                var order = GetMostRecentOrder(request.CustomerId);

                // If no order exists, return customer details with an empty order
                if (order == null)
                {
                    return Ok(new
                    {
                        customer = new { customer.FirstName, customer.LastName },
                        order = new { }
                    });
                }

                // If the order contains a gift, replace product names with "Gift"
                if (order.ContainsGift)
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.OrderItemId = "Gift";
                    }
                }

                // Return the customer and order details
                return Ok(new
                {
                    customer = new { customer.FirstName, customer.LastName },
                    order = new
                    {
                        order.OrderId,
                        order.CustomerId,
                        order.Orderdate,
                        order.OrderItems,
                        order.DeliveryExpected
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });

            }
        }

        private Customers GetCustomerDetails(string customerId, string email)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {

                    dbConnection.Open();

                    var query = "SELECT FirstName, LastName, Email FROM Customers WHERE CustomerId = @CustomerId AND Email = @Email";
                    var customer = dbConnection.Query<Customers>(query, new { CustomerId = customerId, Email = email }).FirstOrDefault();

                    return customer;
                }
                catch (Exception ex)
                {
                    
                    return null;
                }

            }
        }

        private Orders GetMostRecentOrder(string customerId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {


                    dbConnection.Open();

                    var query = @"
                    SELECT TOP 1 o.OrderNumber, o.OrderDate, o.DeliveryAddress, o.DeliveryExpected, o.ContainsGift
                    FROM Orders o
                    WHERE o.CustomerId = @CustomerId
                    ORDER BY o.OrderDate DESC"
                    ;

                    var order = dbConnection.Query<Orders>(query, new { CustomerId = customerId }).FirstOrDefault();

                    if (order != null)
                    {
                        var orderItemsQuery = @"
                        SELECT Product, Quantity, PriceEach
                        FROM OrderItems oi
                        WHERE oi.OrderNumber = @OrderNumber";

                        order.OrderItems = dbConnection.Query<OrderItem>(orderItemsQuery, new { OrderNumber = order.OrderItems }).ToList();
                    }

                    return order;
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }
            
            public class CustomerRequest
        {
            
            public string? CustomerId { get; set; }
            public string? FirstName { get; set; }
    }
    }

    }



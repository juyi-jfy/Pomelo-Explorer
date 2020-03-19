﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Pomelo.Explorer.Definitions;
using Pomelo.Explorer.MySQL.Models;

namespace Pomelo.Explorer.MySQL.Controllers
{
    public class MySQLController : Controller
    {
        [HttpPost]
        public IActionResult CreateConnection(CreateConnectionRequest request)
        {
            var client = new MySqlConnection($"Server={request.Address}; Port={request.Port}; Uid={request.Username}; Pwd={request.Password}; Pooling=False");
            var timestamp = DateTime.UtcNow.Ticks.ToString();
            ConnectionHelper.Connections.Add(timestamp, client);
            return Json(new CreateConnectionResponse 
            {
                Id = timestamp
            });
        }

        [HttpPost]
        public async Task<IActionResult> OpenConnection(string id)
        {
            if (!ConnectionHelper.Connections.ContainsKey(id))
            {
                return NotFound(id);
            }

            try
            {
                await ConnectionHelper.Connections[id].OpenAsync();
            }
            catch (MySqlException ex)
            {
                Response.StatusCode = 400;
                return Json(new DBError 
                {
                    Code = ex.Number,
                    Message = ex.Message
                });
            }

            return Json("OK");
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NetApi.Models;

namespace NetApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        public ITodoRepository TodoItems;
        public TodoController(ITodoRepository todoRepository)
        {
            TodoItems = todoRepository;
        }
        public IEnumerable<TodoItem> GetAll()
        {
            return TodoItems.GetAll();
        }
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            var item = TodoItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody]TodoItem todoItem)
        {
            if (todoItem == null)
            {
                return BadRequest();
            }
            TodoItems.Add(todoItem);
            return CreatedAtAction("GetTodo", new { Controller = "Todo", id = todoItem.Key }, todoItem);
        }


    }
}

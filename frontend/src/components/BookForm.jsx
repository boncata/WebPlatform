import { useState } from "react";

import { createBook } from "../api/books";

function BookForm({ onBookCreated }) {
  const [form, setForm] = useState({
    title: "",
    author: "",
    price: ""
  });

  const handleChange = (event) => {
    setForm({
      ...form, // means: copy all existing fields, override changed field.
      [event.target.name]: event.target.value
    });
  };

  const handleSubmit = async (event) => {
    // Prevents browser from reloading the page. Important!
    event.preventDefault();

    if (!form.title || !form.author || !form.price) {
      alert("All fields are required.");
      return;
    }

    // Set up data to be sent to the backend.
    const payload = {
      title: form.title,
      author: form.author,
      price: Number(form.price)
    };

    try {
    
      await createBook(payload);

      // Reset the form after the successful request.
      setForm({
        title: "",
        author: "",
        price: ""
      });

      onBookCreated(); // Tells parent component that a book is created.
    } catch (error) {
      console.error("Error creating book:", error);
    }
  };

  return (
    // When form submitted, run handleSubmit.
    <form onSubmit={handleSubmit}>
      <input
        name="title"
        placeholder="Title"
        value={form.title}
        onChange={handleChange}
      />

      <input
        name="author"
        placeholder="Author"
        value={form.author}
        onChange={handleChange}
      />

      <input
        name="price"
        placeholder="Price"
        value={form.price}
        onChange={handleChange}
      />

      <button type="submit">
        Add Book
      </button>
    </form>
  );
}

export default BookForm;
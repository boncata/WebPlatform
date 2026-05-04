import { useEffect, useState } from "react";
import axios from "axios";

function App() {

  // Add Form State
  const [books, setBooks] = useState([]);

  const [form, setForm] = useState({
    title: "",
    author: "",
    price: ""
  });

  const api = "http://localhost:5130/api/books";

  // Load Books
  const loadBooks = () => {
    axios.get(api).then((res) => setBooks(res.data));
  };

  useEffect(() => {
    loadBooks();
  }, []);

  // Handle Input Changes
  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value
    });
  };

  //Handle Submit (POST)
  const handleSubmit = async (e) => {
    e.preventDefault();

    // Small input validation
    if (!form.title || !form.author || !form.price) {
      alert("All fields are required");
      return;
    }

    const payload = {
      title: form.title,
      author: form.author,
      price: Number(form.price)
    };

    try {
      await axios.post(api, payload);

      // reset form
      setForm({
        title: "",
        author: "",
        price: ""
      });

      // reload books
      loadBooks();
    } catch (error) {
      console.error("Error creating book:", error);
    }
  };

// Add the Form UI
  return (
    <div>
      <h1>Books</h1>

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

        <button type="submit">Add Book</button>
      </form>

      <ul>
        {books.map((b) => (
          <li key={b.id}>
            {b.title} — {b.author} (${b.price})
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;
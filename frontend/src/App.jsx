import { useEffect, useState } from "react";

import BookList from "./components/BookList";
import BookForm from "./components/BookForm";

import { getBooks } from "./api/books";

/**
 * The main compoment.
 * @returns Returns the specified UI.
 */
function App() {
  // books is the state variable, setBooks is the function that
  // updates the state variable. They both come from useState.
  // useState is given by React and runs React procedures automatically.
  const [books, setBooks] = useState([]);

  const loadBooks = async () => {
    try {
      const data = await getBooks();
      setBooks(data.items);
    } catch (error) {
      console.error("Error loading books:", error);
    }
  };

  useEffect(() => {
    loadBooks();
  }, []);

  return (
    <div>
      <h1>The first 10 books in the database</h1>

      <BookForm onBookCreated={loadBooks} />

      <BookList books={books} />
    </div>
  );
}

export default App;
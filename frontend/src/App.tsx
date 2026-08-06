import { useEffect, useState } from "react";

import BookList from "./components/BookList";
import BookForm from "./components/BookForm";

import { getBooks } from "./api/books";
import type { Book } from "./types/book";

/**
 * The main compoment.
 * @returns Returns the specified UI.
 */
function App() {
  // books is the state variable, setBooks is the function that
  // updates the state variable. They both come from useState.
  // useState is given by React and runs React procedures automatically.
  // <Book[]> tells useState what type the state holds, since an empty
  // array alone isn't enough for TypeScript to infer it.
  const [books, setBooks] = useState<Book[]>([]);

  const loadBooks = async () => {
    try {
      const data = await getBooks();
      setBooks(data.items);
    } catch (error) {
      console.error("Error loading books:", error);
    }
  };

  useEffect(() => {
    // This data-fetching pattern (an async call that sets state directly
    // inside an effect) is being replaced wholesale by a TanStack Query
    // useQuery call in the next phase, which handles this case correctly,
    // so it isn't being restructured here.
    // eslint-disable-next-line react-hooks/set-state-in-effect
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

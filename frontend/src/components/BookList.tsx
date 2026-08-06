import type { Book } from "../types/book";

// Only the fields this component actually renders — narrower than the full
// Book type, so callers (and tests) aren't forced to supply fields that
// have nothing to do with what's displayed here.
type BookListItem = Pick<Book, "id" | "title" | "author" | "price">;

interface BookListProps {
  books: BookListItem[];
}

/**
 * Function to return all the books in the database
 * in HTML format. This function is defined as a React component.
 * A component can be seen as a function that returns UI.
 * @returns HTML formatting of the book list.
 */
function BookList({ books }: BookListProps) {
  // Transform each book element into UI. Lists in React
  // require a unique element, hence <li key={book.id}...
  // Stuff inside curly brackets is read as JavaScript. The rest
  // is considered HTML. This is how JSX works.
  return (
    <ul>
      {books.map((book) => (
        <li key={book.id}>
          {book.title} — {book.author} (€{book.price})
        </li>
      ))}
    </ul>
  );
}

export default BookList;

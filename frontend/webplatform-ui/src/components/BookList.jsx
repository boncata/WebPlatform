/**
 * Function to return all the books in the database
 * in HTML format. This function is defined as a React component.
 * A component can be seen as a function that returns UI.
 * @param {*} books Parameter sent in a format for destructuring. 
 * @returns HTML formatting of the book list.
 */
function BookList({ books }) {
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
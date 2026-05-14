import { describe, test, expect } from "vitest";

import { render, screen } from "@testing-library/react";

import BookList from "./BookList";

// "describe" groups related tests, e.g. a test suite.
describe("BookList", () => {

  /**
   * Test that books are rendered correctly. This is important, because if
   * the data is not rendered correctly, users won't see the correct information.
   * We check that the text content of the rendered component contains the
   * expected book information.
   */
  test("renders books correctly", () => {
    const books = [
      {
        id: 1,
        title: "Clean Code",
        author: "Robert C. Martin",
        price: 30
      },
      {
        id: 2,
        title: "Refactoring",
        author: "Martin Fowler",
        price: 40
      }
    ];

    // Render component into test DOM (not real)
    render(<BookList books={books} />);

    expect(screen.getByText("Clean Code — Robert C. Martin (€30)"))
      .toBeInTheDocument();

    expect(screen.getByText("Refactoring — Martin Fowler (€40)"))
      .toBeInTheDocument();
  });
});
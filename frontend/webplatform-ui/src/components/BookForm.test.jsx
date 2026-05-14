// vi is Vitest utility namespace, similar to mocking helpers.
import { describe, test, expect, vi } from "vitest";

import { render, screen, fireEvent, waitFor } from "@testing-library/react";

import BookForm from "./BookForm";

// Mock the createBook function, because it has a real axios api call, i.e.
// when BookForm imports this module, replace it with fake version.
vi.mock("../api/books", () => {
  return {
    createBook: vi.fn(() => Promise.resolve())
  };
});


describe("BookForm", () => {
  test("calls callback after successful submit", async () => {
    // Create mock callback function. The mock also gathers test info.
    const mockOnBookCreated = vi.fn();

    // Renders Test (fake) DOM and containing BookForm and passes
    // mocked function to it.
    render(
      <BookForm onBookCreated={mockOnBookCreated} />
    );

    // Hint: Real browser input events look like: event.target.value.
    // React receives same structure. 

    // Simulate user typing. 
    fireEvent.change(
      screen.getByPlaceholderText("Title"), // finds <input placeholder="Title" />
      // This is the event object, which simulates browser event.
      // Equivalent to the user typing "Clean Code".
      {
        target: { value: "Clean Code" }
      }
    );

    fireEvent.change(
      screen.getByPlaceholderText("Author"),
      {
        target: { value: "Robert C. Martin" }
      }
    );

    fireEvent.change(
      screen.getByPlaceholderText("Price"),
      {
        target: { value: "30" }
      }
    );

    // Simulates click of a button.
    fireEvent.click(
      screen.getByText("Add Book")
    );

    // Verify that callback function was executed.
    // "waitFor" is needed, because form submission is asynchronous.
    // It will keep checking until assertion passes or timeout occurs.
    // If we use "expect" the assertion might run before async code finishes. 
    await waitFor(() => {
        expect(mockOnBookCreated).toHaveBeenCalled();
    });
  });
});
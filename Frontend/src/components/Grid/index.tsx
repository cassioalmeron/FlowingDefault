import React from 'react';

export interface GridColumn<T> {
  header: string;
  accessor: keyof T;
}

interface GridProps<T> {
  columns: GridColumn<T>[];
  data: T[];
  renderActions?: (row: T) => React.ReactNode;
}

function Grid<T extends { id: number | string }>({ columns, data, renderActions }: GridProps<T>) {
  return (
    <table className="common-table">
      <thead>
        <tr>
          {columns.map(col => (
            <th key={col.accessor as string}>{col.header}</th>
          ))}
          {renderActions && <th className="common-actions-header">Actions</th>}
        </tr>
      </thead>
      <tbody>
        {data.map(row => (
          <tr key={row.id}>
            {columns.map(col => (
              <td key={col.accessor as string}>{String(row[col.accessor])}</td>
            ))}
            {renderActions && (
              <td className="common-actions-cell">{renderActions(row)}</td>
            )}
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default Grid;
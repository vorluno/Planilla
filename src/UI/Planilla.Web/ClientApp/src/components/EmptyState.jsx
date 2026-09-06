import React from 'react';

// Estado vacío. El título estaba en text-gray-900 —casi negro sobre el fondo
// navy-950, también casi negro—: era el texto que orienta a quien llega por
// primera vez y era justo el que no se leía. Resto de cuando la aplicación
// tenía tema claro.

const EmptyState = ({
    icon,
    title,
    description,
    action
}) => {
    return (
        <div className="text-center py-12 px-4">
            {icon && (
                <div className="flex justify-center mb-4">
                    {icon}
                </div>
            )}
            {title && (
                <h3 className="text-lg font-medium text-gray-100 mb-2">
                    {title}
                </h3>
            )}
            {description && (
                <p className="text-gray-400 mb-6 max-w-md mx-auto">
                    {description}
                </p>
            )}
            {action && (
                <div className="flex justify-center">
                    {action}
                </div>
            )}
        </div>
    );
};

export default EmptyState;

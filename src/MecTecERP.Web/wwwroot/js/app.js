// MecTecERP - Arquivo JavaScript Principal

// Inicialização da aplicação
window.MecTecERP = {
    // Configurações globais
    config: {
        dateFormat: 'dd/MM/yyyy',
        currencyFormat: 'pt-BR',
        currency: 'BRL'
    },

    // Utilitários
    utils: {
        // Formatar moeda
        formatCurrency: function(value) {
            return new Intl.NumberFormat('pt-BR', {
                style: 'currency',
                currency: 'BRL'
            }).format(value);
        },

        // Formatar data
        formatDate: function(date) {
            if (!date) return '';
            const d = new Date(date);
            return d.toLocaleDateString('pt-BR');
        },

        // Formatar data e hora
        formatDateTime: function(date) {
            if (!date) return '';
            const d = new Date(date);
            return d.toLocaleString('pt-BR');
        },

        // Validar CNPJ
        validateCNPJ: function(cnpj) {
            cnpj = cnpj.replace(/[^\d]+/g, '');
            
            if (cnpj.length !== 14) return false;
            
            // Elimina CNPJs inválidos conhecidos
            if (/^(\d)\1{13}$/.test(cnpj)) return false;
            
            // Valida DVs
            let tamanho = cnpj.length - 2;
            let numeros = cnpj.substring(0, tamanho);
            let digitos = cnpj.substring(tamanho);
            let soma = 0;
            let pos = tamanho - 7;
            
            for (let i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2) pos = 9;
            }
            
            let resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0)) return false;
            
            tamanho = tamanho + 1;
            numeros = cnpj.substring(0, tamanho);
            soma = 0;
            pos = tamanho - 7;
            
            for (let i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2) pos = 9;
            }
            
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            return resultado == digitos.charAt(1);
        },

        // Aplicar máscara de CNPJ
        maskCNPJ: function(value) {
            value = value.replace(/\D/g, '');
            value = value.replace(/(\d{2})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d)/, '$1/$2');
            value = value.replace(/(\d{4})(\d)/, '$1-$2');
            return value;
        },

        // Aplicar máscara de telefone
        maskPhone: function(value) {
            value = value.replace(/\D/g, '');
            if (value.length <= 10) {
                value = value.replace(/(\d{2})(\d)/, '($1) $2');
                value = value.replace(/(\d{4})(\d)/, '$1-$2');
            } else {
                value = value.replace(/(\d{2})(\d)/, '($1) $2');
                value = value.replace(/(\d{5})(\d)/, '$1-$2');
            }
            return value;
        },

        // Aplicar máscara de CEP
        maskCEP: function(value) {
            value = value.replace(/\D/g, '');
            value = value.replace(/(\d{5})(\d)/, '$1-$2');
            return value;
        },

        // Debounce para pesquisas
        debounce: function(func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        }
    },

    // Funções de UI
    ui: {
        // Mostrar loading
        showLoading: function(element) {
            if (element) {
                element.innerHTML = '<div class="spinner-border spinner-border-sm" role="status"><span class="visually-hidden">Carregando...</span></div>';
                element.disabled = true;
            }
        },

        // Esconder loading
        hideLoading: function(element, originalText) {
            if (element) {
                element.innerHTML = originalText || 'Salvar';
                element.disabled = false;
            }
        },

        // Confirmar ação
        confirm: function(message, callback) {
            if (confirm(message)) {
                callback();
            }
        },

        // Scroll suave para elemento
        scrollTo: function(elementId) {
            const element = document.getElementById(elementId);
            if (element) {
                element.scrollIntoView({ behavior: 'smooth' });
            }
        }
    },

    // Funções de formulário
    forms: {
        // Aplicar máscaras automaticamente
        applyMasks: function() {
            // CNPJ
            document.querySelectorAll('[data-mask="cnpj"]').forEach(input => {
                input.addEventListener('input', function(e) {
                    e.target.value = MecTecERP.utils.maskCNPJ(e.target.value);
                });
            });

            // Telefone
            document.querySelectorAll('[data-mask="phone"]').forEach(input => {
                input.addEventListener('input', function(e) {
                    e.target.value = MecTecERP.utils.maskPhone(e.target.value);
                });
            });

            // CEP
            document.querySelectorAll('[data-mask="cep"]').forEach(input => {
                input.addEventListener('input', function(e) {
                    e.target.value = MecTecERP.utils.maskCEP(e.target.value);
                });
            });

            // Moeda
            document.querySelectorAll('[data-mask="currency"]').forEach(input => {
                input.addEventListener('input', function(e) {
                    let value = e.target.value.replace(/\D/g, '');
                    value = (value / 100).toFixed(2);
                    e.target.value = value.replace('.', ',');
                });
            });
        },

        // Validar formulário
        validate: function(formElement) {
            let isValid = true;
            const inputs = formElement.querySelectorAll('input[required], select[required], textarea[required]');
            
            inputs.forEach(input => {
                if (!input.value.trim()) {
                    input.classList.add('is-invalid');
                    isValid = false;
                } else {
                    input.classList.remove('is-invalid');
                    input.classList.add('is-valid');
                }
            });

            return isValid;
        }
    },

    // Funções de tabela
    tables: {
        // Aplicar filtros
        applyFilters: function(tableId, filters) {
            const table = document.getElementById(tableId);
            if (!table) return;

            const rows = table.querySelectorAll('tbody tr');
            
            rows.forEach(row => {
                let showRow = true;
                
                Object.keys(filters).forEach(key => {
                    const filterValue = filters[key].toLowerCase();
                    if (filterValue) {
                        const cell = row.querySelector(`[data-filter="${key}"]`);
                        if (cell && !cell.textContent.toLowerCase().includes(filterValue)) {
                            showRow = false;
                        }
                    }
                });
                
                row.style.display = showRow ? '' : 'none';
            });
        },

        // Ordenar tabela
        sortTable: function(tableId, columnIndex, direction = 'asc') {
            const table = document.getElementById(tableId);
            if (!table) return;

            const tbody = table.querySelector('tbody');
            const rows = Array.from(tbody.querySelectorAll('tr'));

            rows.sort((a, b) => {
                const aVal = a.cells[columnIndex].textContent.trim();
                const bVal = b.cells[columnIndex].textContent.trim();
                
                if (direction === 'asc') {
                    return aVal.localeCompare(bVal, 'pt-BR', { numeric: true });
                } else {
                    return bVal.localeCompare(aVal, 'pt-BR', { numeric: true });
                }
            });

            rows.forEach(row => tbody.appendChild(row));
        }
    }
};

// Inicialização quando o DOM estiver carregado
document.addEventListener('DOMContentLoaded', function() {
    // Aplicar máscaras automaticamente
    MecTecERP.forms.applyMasks();
    
    // Configurar tooltips do Bootstrap
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Configurar popovers do Bootstrap
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});

// Funções globais para interoperabilidade com Blazor
window.blazorCulture = {
    get: () => window.localStorage['BlazorCulture'],
    set: (value) => window.localStorage['BlazorCulture'] = value
};

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
};

window.copyToClipboard = (text) => {
    navigator.clipboard.writeText(text).then(() => {
        console.log('Texto copiado para a área de transferência');
    }).catch(err => {
        console.error('Erro ao copiar texto: ', err);
    });
};

window.printElement = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`
            <html>
                <head>
                    <title>Impressão</title>
                    <link href="/css/bootstrap.min.css" rel="stylesheet">
                    <link href="/css/app.css" rel="stylesheet">
                    <style>
                        @media print {
                            .no-print { display: none !important; }
                            body { margin: 0; }
                        }
                    </style>
                </head>
                <body>
                    ${element.outerHTML}
                </body>
            </html>
        `);
        printWindow.document.close();
        printWindow.focus();
        printWindow.print();
        printWindow.close();
    }
};
// MecTecERP - Sistema de Notificações

window.NotificacaoSystem = {
    // Configurações
    config: {
        checkInterval: 30000, // 30 segundos
        maxNotifications: 50,
        autoMarkAsRead: true,
        soundEnabled: true
    },

    // Estado das notificações
    state: {
        notifications: [],
        unreadCount: 0,
        isConnected: false,
        lastCheck: null
    },

    // Elementos DOM
    elements: {
        badge: null,
        dropdown: null,
        list: null,
        markAllRead: null
    },

    // Inicializar sistema
    init: function() {
        this.initElements();
        this.setupEventListeners();
        this.startPeriodicCheck();
        this.loadNotifications();
    },

    // Inicializar elementos DOM
    initElements: function() {
        this.elements.badge = document.getElementById('notificationBadge');
        this.elements.dropdown = document.getElementById('notificationDropdown');
        this.elements.list = document.getElementById('notificationList');
        this.elements.markAllRead = document.getElementById('markAllAsRead');
    },

    // Configurar event listeners
    setupEventListeners: function() {
        // Marcar todas como lidas
        if (this.elements.markAllRead) {
            this.elements.markAllRead.addEventListener('click', () => {
                this.markAllAsRead();
            });
        }

        // Clique em notificação individual
        if (this.elements.list) {
            this.elements.list.addEventListener('click', (e) => {
                const notificationItem = e.target.closest('.notification-item');
                if (notificationItem) {
                    const notificationId = notificationItem.dataset.id;
                    this.markAsRead(notificationId);
                }
            });
        }

        // Fechar dropdown ao clicar fora
        document.addEventListener('click', (e) => {
            if (this.elements.dropdown && !this.elements.dropdown.contains(e.target)) {
                this.closeDropdown();
            }
        });
    },

    // Iniciar verificação periódica
    startPeriodicCheck: function() {
        setInterval(() => {
            this.checkForNewNotifications();
        }, this.config.checkInterval);
    },

    // Carregar notificações
    loadNotifications: function() {
        // Simular carregamento de notificações
        // Em produção, isso viria de uma API
        const mockNotifications = this.generateMockNotifications();
        this.updateNotifications(mockNotifications);
    },

    // Gerar notificações de exemplo
    generateMockNotifications: function() {
        const types = [
            { type: 'estoque_baixo', icon: 'fas fa-exclamation-triangle', color: 'warning' },
            { type: 'estoque_zerado', icon: 'fas fa-times-circle', color: 'danger' },
            { type: 'movimentacao', icon: 'fas fa-exchange-alt', color: 'info' },
            { type: 'inventario', icon: 'fas fa-clipboard-list', color: 'primary' },
            { type: 'sistema', icon: 'fas fa-cog', color: 'secondary' }
        ];

        const messages = {
            estoque_baixo: [
                'Produto "Chave Phillips 1/4" está com estoque baixo (3 unidades)',
                'Produto "Parafuso M6" está com estoque baixo (8 unidades)',
                'Produto "Óleo Lubrificante" está com estoque baixo (2 unidades)'
            ],
            estoque_zerado: [
                'Produto "Filtro de Ar" está com estoque zerado',
                'Produto "Correia Dentada" está com estoque zerado'
            ],
            movimentacao: [
                'Nova entrada de estoque registrada para "Chave de Fenda"',
                'Saída de estoque registrada para "Parafuso M8"',
                'Movimentação de ajuste realizada no produto "Óleo Motor"'
            ],
            inventario: [
                'Inventário "INV-2024-001" foi finalizado',
                'Novo inventário "INV-2024-002" foi iniciado',
                'Divergências encontradas no inventário "INV-2024-001"'
            ],
            sistema: [
                'Sistema atualizado para versão 1.2.0',
                'Backup automático realizado com sucesso',
                'Manutenção programada para domingo às 02:00'
            ]
        };

        const notifications = [];
        
        for (let i = 0; i < 15; i++) {
            const type = types[Math.floor(Math.random() * types.length)];
            const messageArray = messages[type.type];
            const message = messageArray[Math.floor(Math.random() * messageArray.length)];
            
            const date = new Date();
            date.setMinutes(date.getMinutes() - Math.floor(Math.random() * 1440)); // Últimas 24 horas
            
            notifications.push({
                id: `notif_${i}`,
                type: type.type,
                icon: type.icon,
                color: type.color,
                title: this.getNotificationTitle(type.type),
                message: message,
                timestamp: date,
                isRead: Math.random() > 0.6, // 40% não lidas
                priority: Math.random() > 0.8 ? 'high' : 'normal'
            });
        }

        return notifications.sort((a, b) => b.timestamp - a.timestamp);
    },

    // Obter título da notificação
    getNotificationTitle: function(type) {
        const titles = {
            estoque_baixo: 'Estoque Baixo',
            estoque_zerado: 'Estoque Zerado',
            movimentacao: 'Movimentação',
            inventario: 'Inventário',
            sistema: 'Sistema'
        };
        return titles[type] || 'Notificação';
    },

    // Atualizar notificações
    updateNotifications: function(notifications) {
        this.state.notifications = notifications;
        this.state.unreadCount = notifications.filter(n => !n.isRead).length;
        this.updateUI();
    },

    // Atualizar interface
    updateUI: function() {
        this.updateBadge();
        this.updateDropdown();
    },

    // Atualizar badge de contagem
    updateBadge: function() {
        if (this.elements.badge) {
            if (this.state.unreadCount > 0) {
                this.elements.badge.textContent = this.state.unreadCount > 99 ? '99+' : this.state.unreadCount;
                this.elements.badge.style.display = 'inline-block';
            } else {
                this.elements.badge.style.display = 'none';
            }
        }
    },

    // Atualizar dropdown
    updateDropdown: function() {
        if (!this.elements.list) return;

        if (this.state.notifications.length === 0) {
            this.elements.list.innerHTML = `
                <div class="dropdown-item text-center text-muted py-3">
                    <i class="fas fa-bell-slash mb-2"></i><br>
                    Nenhuma notificação
                </div>
            `;
            return;
        }

        const html = this.state.notifications.map(notification => {
            const timeAgo = this.getTimeAgo(notification.timestamp);
            const readClass = notification.isRead ? 'read' : 'unread';
            const priorityClass = notification.priority === 'high' ? 'priority-high' : '';

            return `
                <div class="dropdown-item notification-item ${readClass} ${priorityClass}" data-id="${notification.id}">
                    <div class="d-flex align-items-start">
                        <div class="notification-icon text-${notification.color} me-3">
                            <i class="${notification.icon}"></i>
                        </div>
                        <div class="notification-content flex-grow-1">
                            <div class="notification-title fw-bold">${notification.title}</div>
                            <div class="notification-message text-muted small">${notification.message}</div>
                            <div class="notification-time text-muted small">
                                <i class="fas fa-clock me-1"></i>${timeAgo}
                            </div>
                        </div>
                        ${!notification.isRead ? '<div class="notification-dot bg-primary"></div>' : ''}
                    </div>
                </div>
            `;
        }).join('');

        this.elements.list.innerHTML = html;
    },

    // Calcular tempo decorrido
    getTimeAgo: function(date) {
        const now = new Date();
        const diffInSeconds = Math.floor((now - date) / 1000);

        if (diffInSeconds < 60) {
            return 'Agora mesmo';
        } else if (diffInSeconds < 3600) {
            const minutes = Math.floor(diffInSeconds / 60);
            return `${minutes} min atrás`;
        } else if (diffInSeconds < 86400) {
            const hours = Math.floor(diffInSeconds / 3600);
            return `${hours}h atrás`;
        } else {
            const days = Math.floor(diffInSeconds / 86400);
            return `${days}d atrás`;
        }
    },

    // Verificar novas notificações
    checkForNewNotifications: function() {
        // Simular verificação de novas notificações
        // Em produção, isso faria uma chamada para a API
        
        if (Math.random() > 0.8) { // 20% de chance de nova notificação
            const newNotification = this.generateRandomNotification();
            this.addNotification(newNotification);
        }
    },

    // Gerar notificação aleatória
    generateRandomNotification: function() {
        const types = [
            { type: 'estoque_baixo', icon: 'fas fa-exclamation-triangle', color: 'warning' },
            { type: 'movimentacao', icon: 'fas fa-exchange-alt', color: 'info' }
        ];

        const type = types[Math.floor(Math.random() * types.length)];
        
        return {
            id: `notif_${Date.now()}`,
            type: type.type,
            icon: type.icon,
            color: type.color,
            title: this.getNotificationTitle(type.type),
            message: 'Nova notificação recebida',
            timestamp: new Date(),
            isRead: false,
            priority: 'normal'
        };
    },

    // Adicionar nova notificação
    addNotification: function(notification) {
        this.state.notifications.unshift(notification);
        
        // Limitar número de notificações
        if (this.state.notifications.length > this.config.maxNotifications) {
            this.state.notifications = this.state.notifications.slice(0, this.config.maxNotifications);
        }
        
        this.state.unreadCount++;
        this.updateUI();
        
        // Tocar som se habilitado
        if (this.config.soundEnabled) {
            this.playNotificationSound();
        }
        
        // Mostrar toast
        this.showToast(notification);
    },

    // Marcar como lida
    markAsRead: function(notificationId) {
        const notification = this.state.notifications.find(n => n.id === notificationId);
        if (notification && !notification.isRead) {
            notification.isRead = true;
            this.state.unreadCount--;
            this.updateUI();
        }
    },

    // Marcar todas como lidas
    markAllAsRead: function() {
        this.state.notifications.forEach(notification => {
            notification.isRead = true;
        });
        this.state.unreadCount = 0;
        this.updateUI();
    },

    // Fechar dropdown
    closeDropdown: function() {
        if (this.elements.dropdown) {
            const dropdown = bootstrap.Dropdown.getInstance(this.elements.dropdown);
            if (dropdown) {
                dropdown.hide();
            }
        }
    },

    // Tocar som de notificação
    playNotificationSound: function() {
        try {
            const audio = new Audio('data:audio/wav;base64,UklGRnoGAABXQVZFZm10IBAAAAABAAEAQB8AAEAfAAABAAgAZGF0YQoGAACBhYqFbF1fdJivrJBhNjVgodDbq2EcBj+a2/LDciUFLIHO8tiJNwgZaLvt559NEAxQp+PwtmMcBjiR1/LMeSwFJHfH8N2QQAoUXrTp66hVFApGn+DyvmwhBSuBzvLZiTYIG2m98OScTgwOUarm7blmGgU7k9n1unEiBC13yO/eizEIHWq+8+OWT');
            audio.volume = 0.3;
            audio.play().catch(() => {
                // Ignorar erro se não conseguir tocar o som
            });
        } catch (error) {
            // Ignorar erro de som
        }
    },

    // Mostrar toast
    showToast: function(notification) {
        if (window.blazorCulture && window.blazorCulture.showToast) {
            window.blazorCulture.showToast(notification.title, notification.message, 'info');
        }
    },

    // Obter configurações
    getConfig: function() {
        return this.config;
    },

    // Atualizar configurações
    updateConfig: function(newConfig) {
        this.config = { ...this.config, ...newConfig };
    },

    // Obter estatísticas
    getStats: function() {
        return {
            total: this.state.notifications.length,
            unread: this.state.unreadCount,
            byType: this.getNotificationsByType()
        };
    },

    // Obter notificações por tipo
    getNotificationsByType: function() {
        const byType = {};
        this.state.notifications.forEach(notification => {
            byType[notification.type] = (byType[notification.type] || 0) + 1;
        });
        return byType;
    }
};

// Inicializar quando o DOM estiver carregado
document.addEventListener('DOMContentLoaded', function() {
    NotificacaoSystem.init();
});

// Funções para interoperabilidade com Blazor
window.initNotificationSystem = function() {
    NotificacaoSystem.init();
};

window.addNotification = function(notification) {
    NotificacaoSystem.addNotification(notification);
};

window.markNotificationAsRead = function(notificationId) {
    NotificacaoSystem.markAsRead(notificationId);
};

window.markAllNotificationsAsRead = function() {
    NotificacaoSystem.markAllAsRead();
};

window.getNotificationStats = function() {
    return NotificacaoSystem.getStats();
};

window.updateNotificationConfig = function(config) {
    NotificacaoSystem.updateConfig(config);
};